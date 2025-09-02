// Live Updates JavaScript for CricHeroes
class LiveMatchUpdater {
    constructor(matchId, updateInterval = 5000) {
        this.matchId = matchId;
        this.updateInterval = updateInterval;
        this.isUpdating = false;
        this.updateTimer = null;
        this.init();
    }

    init() {
        this.startUpdates();
        this.setupEventListeners();
    }

    startUpdates() {
        if (this.isUpdating) return;
        
        this.isUpdating = true;
        this.updateTimer = setInterval(() => {
            this.fetchLiveScore();
        }, this.updateInterval);
        
        console.log(`Live updates started for match ${this.matchId}`);
    }

    stopUpdates() {
        if (this.updateTimer) {
            clearInterval(this.updateTimer);
            this.updateTimer = null;
        }
        this.isUpdating = false;
        console.log(`Live updates stopped for match ${this.matchId}`);
    }

    async fetchLiveScore() {
        try {
            const response = await fetch(`/Live/LiveScore?matchId=${this.matchId}`);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const liveData = await response.json();
            this.updateUI(liveData);
        } catch (error) {
            console.error('Error fetching live score:', error);
        }
    }

    updateUI(liveData) {
        // Update score displays
        this.updateElement('current-runs', liveData.CurrentRuns);
        this.updateElement('current-wickets', liveData.CurrentWickets);
        this.updateElement('current-overs', liveData.CurrentOvers);
        this.updateElement('target-runs', liveData.TargetRuns);
        this.updateElement('batting-team', liveData.BattingTeam);
        this.updateElement('bowling-team', liveData.BowlingTeam);
        this.updateElement('last-ball', liveData.LastBall);
        this.updateElement('last-updated', new Date(liveData.LastUpdated).toLocaleTimeString());

        // Update match status if needed
        if (liveData.Status) {
            this.updateElement('match-status', liveData.Status);
        }

        // Add visual indicator for live updates
        this.showLiveIndicator();
    }

    updateElement(className, value) {
        const elements = document.querySelectorAll(`.${className}`);
        elements.forEach(element => {
            if (element.textContent !== value.toString()) {
                element.textContent = value;
                element.classList.add('updated');
                setTimeout(() => element.classList.remove('updated'), 1000);
            }
        });
    }

    showLiveIndicator() {
        const indicator = document.getElementById('live-indicator');
        if (indicator) {
            indicator.style.display = 'block';
            indicator.classList.add('pulse');
            setTimeout(() => indicator.classList.remove('pulse'), 1000);
        }
    }

    setupEventListeners() {
        // Pause updates when page is not visible
        document.addEventListener('visibilitychange', () => {
            if (document.hidden) {
                this.stopUpdates();
            } else {
                this.startUpdates();
            }
        });

        // Stop updates when leaving the page
        window.addEventListener('beforeunload', () => {
            this.stopUpdates();
        });
    }
}

// Ball by Ball Commentary Manager
class CommentaryManager {
    constructor(containerId) {
        this.container = document.getElementById(containerId);
        this.commentaryItems = [];
        this.init();
    }

    init() {
        if (this.container) {
            this.setupAutoScroll();
        }
    }

    addCommentary(ball) {
        const commentaryItem = this.createCommentaryItem(ball);
        this.container.insertBefore(commentaryItem, this.container.firstChild);
        
        // Keep only last 50 commentary items
        if (this.container.children.length > 50) {
            this.container.removeChild(this.container.lastChild);
        }

        this.commentaryItems.unshift(ball);
        this.autoScroll();
    }

    createCommentaryItem(ball) {
        const item = document.createElement('div');
        item.className = 'commentary-item new-commentary';
        
        const ballHeader = document.createElement('div');
        ballHeader.className = 'ball-header';
        ballHeader.innerHTML = `
            <span class="ball-number">${ball.OverNumber}.${ball.BallNumber}</span>
            <span class="ball-time">${new Date(ball.Timestamp).toLocaleTimeString()}</span>
        `;

        const ballContent = document.createElement('div');
        ballContent.className = 'ball-content';
        
        if (ball.IsWicket) {
            ballContent.innerHTML = `
                <span class="ball-result wicket">
                    <i class="fas fa-times-circle"></i> WICKET!
                </span>
            `;
        } else if (ball.IsWide) {
            ballContent.innerHTML = `
                <span class="ball-result wide">
                    <i class="fas fa-arrow-right"></i> WIDE +${ball.ExtraRuns}
                </span>
            `;
        } else if (ball.IsNoBall) {
            ballContent.innerHTML = `
                <span class="ball-result noball">
                    <i class="fas fa-exclamation-triangle"></i> NO BALL +${ball.ExtraRuns}
                </span>
            `;
        } else {
            ballContent.innerHTML = `
                <span class="ball-result runs">
                    ${ball.Runs} runs
                </span>
            `;
        }

        item.appendChild(ballHeader);
        item.appendChild(ballContent);

        if (ball.Commentary) {
            const commentary = document.createElement('div');
            commentary.className = 'ball-commentary';
            commentary.innerHTML = `<small class="text-muted">${ball.Commentary}</small>`;
            item.appendChild(commentary);
        }

        // Remove new commentary class after animation
        setTimeout(() => item.classList.remove('new-commentary'), 2000);

        return item;
    }

    setupAutoScroll() {
        this.container.addEventListener('scroll', () => {
            const isAtBottom = this.container.scrollTop + this.container.clientHeight >= this.container.scrollHeight - 10;
            this.autoScrollEnabled = isAtBottom;
        });
    }

    autoScroll() {
        if (this.autoScrollEnabled !== false) {
            this.container.scrollTop = 0;
        }
    }

    clearCommentary() {
        this.container.innerHTML = '';
        this.commentaryItems = [];
    }
}

// Score Calculator
class ScoreCalculator {
    static calculateOvers(balls) {
        const totalBalls = balls.length;
        const overs = Math.floor(totalBalls / 6);
        const remainingBalls = totalBalls % 6;
        return overs + (remainingBalls / 10);
    }

    static calculateRunRate(runs, overs) {
        if (overs === 0) return 0;
        return (runs / overs).toFixed(2);
    }

    static calculateRequiredRunRate(target, currentRuns, overs) {
        const remainingOvers = 20 - overs;
        if (remainingOvers <= 0) return 0;
        const requiredRuns = target - currentRuns;
        return (requiredRuns / remainingOvers).toFixed(2);
    }
}

// Utility Functions
const LiveUtils = {
    formatTime: (date) => {
        return new Date(date).toLocaleTimeString('en-US', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
    },

    formatOvers: (overs) => {
        const wholeOvers = Math.floor(overs);
        const balls = Math.round((overs - wholeOvers) * 10);
        return `${wholeOvers}.${balls}`;
    },

    animateValue: (element, start, end, duration = 1000) => {
        const startTime = performance.now();
        const animate = (currentTime) => {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            
            const current = Math.round(start + (end - start) * progress);
            element.textContent = current;
            
            if (progress < 1) {
                requestAnimationFrame(animate);
            }
        };
        requestAnimationFrame(animate);
    },

    showNotification: (message, type = 'info') => {
        const notification = document.createElement('div');
        notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
        notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
        notification.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(notification);
        
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 5000);
    }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize live updates for all matches on the page
    const matchElements = document.querySelectorAll('[data-match-id]');
    matchElements.forEach(element => {
        const matchId = element.getAttribute('data-match-id');
        if (matchId) {
            new LiveMatchUpdater(parseInt(matchId));
        }
    });

    // Initialize commentary manager if present
    const commentaryContainer = document.getElementById('commentary-container');
    if (commentaryContainer) {
        window.commentaryManager = new CommentaryManager('commentary-container');
    }

    console.log('Live Updates initialized');
});
